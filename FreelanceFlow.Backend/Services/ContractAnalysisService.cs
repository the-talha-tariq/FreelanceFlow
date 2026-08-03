using AutoMapper;
using FreelanceFlow.Backend.DTOs.Contracts;
using FreelanceFlow.Backend.ExternalServices;
using FreelanceFlow.Backend.Helpers;
using FreelanceFlow.Backend.Models.Entities;
using FreelanceFlow.Backend.Repositories.Interfaces;
using FreelanceFlow.Backend.Services.Interfaces;

namespace FreelanceFlow.Backend.Services;

public class ContractAnalysisService : IContractAnalysisService
{
    private readonly IContractRepository _contractRepository;
    private readonly IContractRiskAnalysisRepository _riskAnalysisRepository;
    private readonly IContractTextExtractionService _textExtractionService;
    private readonly IOpenAIContractAnalyzerService _aiAnalyzer;
    private readonly IMapper _mapper;

    public ContractAnalysisService(
        IContractRepository contractRepository,
        IContractRiskAnalysisRepository riskAnalysisRepository,
        IContractTextExtractionService textExtractionService,
        IOpenAIContractAnalyzerService aiAnalyzer,
        IMapper mapper)
    {
        _contractRepository = contractRepository;
        _riskAnalysisRepository = riskAnalysisRepository;
        _textExtractionService = textExtractionService;
        _aiAnalyzer = aiAnalyzer;
        _mapper = mapper;
    }

    public async Task<ServiceResult<IReadOnlyList<RiskAnalysisDto>>> AnalyzeAsync(Guid freelancerId, Guid contractId)
    {
        var contract = await _contractRepository.GetByIdAsync(contractId);
        if (contract == null || contract.FreelancerId != freelancerId)
        {
            return ServiceResult<IReadOnlyList<RiskAnalysisDto>>.FailureResult("Contract not found.");
        }

        if (string.IsNullOrEmpty(contract.DocumentPath))
        {
            return ServiceResult<IReadOnlyList<RiskAnalysisDto>>.FailureResult(
                "This contract has no uploaded document. Upload one via POST /api/contracts/{id}/document first.");
        }

        string contractText;
        try
        {
            contractText = await _textExtractionService.ExtractTextAsync(contract.DocumentPath);
        }
        catch (NotSupportedException ex)
        {
            return ServiceResult<IReadOnlyList<RiskAnalysisDto>>.FailureResult(ex.Message);
        }
        catch (FileNotFoundException)
        {
            return ServiceResult<IReadOnlyList<RiskAnalysisDto>>.FailureResult(
                "The contract document is missing from storage. Try re-uploading it.");
        }

        if (string.IsNullOrWhiteSpace(contractText))
        {
            return ServiceResult<IReadOnlyList<RiskAnalysisDto>>.FailureResult(
                "No extractable text was found in the contract document.");
        }

        ExternalServices.Models.ContractAnalysisResult analysisResult;
        try
        {
            analysisResult = await _aiAnalyzer.AnalyzeContractTextAsync(contractText);
        }
        catch (Exception ex)
        {
            return ServiceResult<IReadOnlyList<RiskAnalysisDto>>.FailureResult(ex.Message);
        }

        // Each /analyze call replaces the previous batch rather than piling
        // up history — GET .../analysis always reflects the latest run.
        var existingRows = await _riskAnalysisRepository.GetByContractIdAsync(contractId);
        foreach (var oldRow in existingRows)
        {
            _riskAnalysisRepository.Remove(oldRow);
        }

        var analyzedAt = DateTime.UtcNow;
        var newRows = analysisResult.Flags.Select(flag => new ContractRiskAnalysis
        {
            Id = Guid.NewGuid(),
            ContractId = contractId,
            AnalyzedAt = analyzedAt,
            RawAIResponse = analysisResult.RawResponse,
            ClauseType = flag.ClauseType,
            Severity = flag.Severity,
            ExtractedText = flag.ExtractedText,
            Explanation = flag.Explanation,
            SuggestedAlternative = flag.SuggestedAlternative
        }).ToList();

        foreach (var newRow in newRows)
        {
            await _riskAnalysisRepository.AddAsync(newRow);
        }

        await _riskAnalysisRepository.SaveChangesAsync();

        return ServiceResult<IReadOnlyList<RiskAnalysisDto>>.SuccessResult(
            _mapper.Map<IReadOnlyList<RiskAnalysisDto>>(newRows));
    }

    public async Task<ServiceResult<IReadOnlyList<RiskAnalysisDto>>> GetAnalysisAsync(Guid freelancerId, Guid contractId)
    {
        var contract = await _contractRepository.GetByIdAsync(contractId);
        if (contract == null || contract.FreelancerId != freelancerId)
        {
            return ServiceResult<IReadOnlyList<RiskAnalysisDto>>.FailureResult("Contract not found.");
        }

        var rows = await _riskAnalysisRepository.GetByContractIdAsync(contractId);
        return ServiceResult<IReadOnlyList<RiskAnalysisDto>>.SuccessResult(
            _mapper.Map<IReadOnlyList<RiskAnalysisDto>>(rows));
    }
}