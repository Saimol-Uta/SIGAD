namespace SIGAD.Domain.Enums
{
    /// <summary>
    /// Estados de las tesis dirigidas
    /// </summary>
    public enum EstadoTesis
    {
        /// <summary>
        /// Tesis en proceso de desarrollo
        /// </summary>
        EnProceso = 1,

        /// <summary>
        /// Tesis culminada y defendida exitosamente
        /// </summary>
        Culminada = 2,

        /// <summary>
        /// Tesis suspendida o abandonada
        /// </summary>
        Suspendida = 3,

        /// <summary>
        /// Tesis aprobada pero pendiente de defensa
        /// </summary>
        AprobadaPendienteDefensa = 4
    }
}
