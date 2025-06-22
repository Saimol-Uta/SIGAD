namespace SIGAD.Domain.Enums
{
    /// <summary>
    /// Tipos de cursos según el reglamento Art. 3d para promoción académica
    /// </summary>
    public enum TipoCurso
    {
        /// <summary>
        /// Cursos de actualización científica en el área de conocimiento
        /// </summary>
        ActualizacionCientifica = 1,

        /// <summary>
        /// Cursos de actualización pedagógica (mínimo 25% del total requerido)
        /// </summary>
        ActualizacionPedagogica = 2,

        /// <summary>
        /// Cursos de especialización profesional
        /// </summary>
        Especializacion = 3,

        /// <summary>
        /// Diplomados y programas de formación continua
        /// </summary>
        Diplomado = 4,

        /// <summary>
        /// Seminarios, talleres y eventos académicos
        /// </summary>
        Seminario = 5,

        /// <summary>
        /// Conferencias magistrales y ponencias
        /// </summary>
        Conferencia = 6,

        /// <summary>
        /// Certificaciones profesionales e internacionales
        /// </summary>
        Certificacion = 7,

        /// <summary>
        /// Cursos de capacitación impartidos por el docente
        /// </summary>
        CapacitacionImpartida = 8
    }
}
