using PropertyManager.Application.DTOs.PriceComponents;

namespace PropertyManager.WEB.ViewModels.PriceComponents
{
    public class PriceComponentIndexViewModel
    {
        public IReadOnlyList<PriceComponentDto> Components { get; set; } = Array.Empty<PriceComponentDto>();
        public PriceComponentFormViewModel Form { get; set; } = new();
    }
}
