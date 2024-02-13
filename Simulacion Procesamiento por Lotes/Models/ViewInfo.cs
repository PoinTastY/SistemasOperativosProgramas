using CommunityToolkit.Mvvm.ComponentModel;

namespace Simulacion_Procesamiento_por_Lotes.Models
{
    public partial class ViewInfo : ObservableObject
    {
        [ObservableProperty]
        private int _id;
        [ObservableProperty]
        private string _instruccion;
        [ObservableProperty]
        private string _programador;
        [ObservableProperty]
        private int _tme;

        public ViewInfo() { }
        public ViewInfo(int id, string instruccion, string programador, int tme)
        {
            _id = id;
            _instruccion = instruccion;
            _programador = programador;
            _tme = tme;
        }

    }
}
