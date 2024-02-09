namespace Simulacion_Procesamiento_por_Lotes.Models
{
    public class Lote
    {
        private readonly int _capacidadMax;
        private int _procesosActuales;
        private readonly List<Proceso> _procesos;

        public Lote(int size)
        {
            _capacidadMax = size;
            _procesos = new(size);
            _procesosActuales = 0;
        }

        //add proceso
        public bool Add(Proceso proceso)
        {
            if (_procesosActuales < _capacidadMax)
            {
                _procesos.Add(proceso);
                _procesosActuales += 1;
                return true;
            }
            else
            {
                return false;
            }
        }

        public Proceso TakeFirst()
        {
            _procesosActuales -= 1;

            if (_procesosActuales != 0)
            {
                // Si hay procesos restantes en la lista:
                Proceso primerProceso = _procesos[0]; // Tomar el primer proceso
                _procesos.RemoveAt(0); // Eliminar el primer proceso de la lista
                return primerProceso; // Elimina y devuelve el primer proceso
            }
            else
            {
                return null; // Si no hay procesos restantes, devuelve null o realiza otra acción según lo necesario
            }
        }

        public List<Proceso> Procesos {get => _procesos; }

        //overload to validate if lote is empty
        public static implicit operator bool(Lote lote)
        {
            return lote._procesosActuales < lote._capacidadMax;
        }

    }
}
