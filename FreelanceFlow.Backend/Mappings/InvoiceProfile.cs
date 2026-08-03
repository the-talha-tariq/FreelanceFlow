using AutoMapper;
using FreelanceFlow.Backend.DTOs.Invoices;
using FreelanceFlow.Backend.Models.Entities;

namespace FreelanceFlow.Backend.Mappings;

public class InvoiceProfile : Profile
{
    public InvoiceProfile()
    {
        CreateMap<Invoice, InvoiceDto>()
            .ForMember(dest => dest.ClientName, opt => opt.MapFrom(src => src.Client.Name));

        CreateMap<Invoice, InvoiceDetailDto>()
            .ForMember(dest => dest.ClientName, opt => opt.MapFrom(src => src.Client.Name))
            .ForMember(dest => dest.AmountPaid, opt => opt.MapFrom(src => src.Payments.Sum(p => p.Amount)));

        CreateMap<InvoiceLineItem, InvoiceLineItemDto>();

        // Line items and computed totals (SubTotal/TaxAmount/TotalAmount)
        // are built explicitly in InvoiceService rather than mapped, since
        // Total per line and the invoice-level sums must be recomputed
        // server-side rather than trusted from client input.
    }
}