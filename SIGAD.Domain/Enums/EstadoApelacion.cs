namespace SIGAD.Domain.Enums
{
    /// <summary>
    /// Estados posibles de una apelación según Artículo 6 del Reglamento UTA
    /// </summary>
    public enum EstadoApelacion
    {
        /// <summary>
        /// Apelación presentada, esperando resolución (3 días máximo según reglamento)
        /// </summary>
        Pendiente = 1,

        /// <summary>
        /// Apelación aceptada por la Comisión Académica de Escalafón y Promoción
        /// </summary>
        Aceptada = 2,

        /// <summary>
        /// Apelación rechazada por la Comisión Académica de Escalafón y Promoción
        /// </summary>
        Rechazada = 3,

        /// <summary>
        /// Apelación vencida (no resuelta en el tiempo establecido)
        /// </summary>
        Vencida = 4
    }
}
