using VegetableSupplier.ViewModels;

namespace VegetableSupplier.Views;

public partial class VegetableEditPage : ContentPage
{
    public VegetableEditPage(VegetableEditViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}