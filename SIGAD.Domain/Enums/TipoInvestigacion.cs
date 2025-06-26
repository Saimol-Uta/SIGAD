namespace SIGAD.Domain.Enums
{
    /// <summary>
    /// Tipos de investigación según Art. 3d del reglamento de promoción
    /// Las investigaciones prioritarias tienen mayor ponderación
    /// </summary>
    public enum TipoInvestigacion
    {
        /// <summary>
        /// Investigación básica (prioritaria según reglamento)
        /// </summary>
        Basica = 1,

        /// <summary>
        /// Investigación aplicada
        /// </summary>
        Aplicada = 2,

        /// <summary>
        /// Investigación experimental
        /// </summary>
        Experimental = 3,

        /// <summary>
        /// Investigación enfocada en grupos vulnerables (prioritaria)
        /// </summary>
        GruposVulnerables = 4,

        /// <summary>
        /// Investigación para atender necesidades sociales (prioritaria)
        /// </summary>
        NecesidadesSociales = 5,

        /// <summary>
        /// Investigación sectorial y productiva (prioritaria)
        /// </summary>
        Productiva = 6,

        /// <summary>
        /// Investigación en zonas rurales (prioritaria)
        /// </summary>
        ZonasRurales = 7,

        /// <summary>
        /// Proyectos de vinculación con la sociedad
        /// </summary>
        Vinculacion = 8,

        /// <summary>
        /// Investigación interdisciplinaria
        /// </summary>
        Interdisciplinaria = 9,

        /// <summary>
        /// Investigación internacional colaborativa
        /// </summary>
        Internacional = 10
    }
}
