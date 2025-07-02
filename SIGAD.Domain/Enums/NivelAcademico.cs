namespace SIGAD.Domain.Enums
{
    /// <summary>
    /// Niveles académicos para tesis dirigidas según requisitos de promoción
    /// </summary>
    public enum NivelAcademico
    {
        /// <summary>
        /// Tesis de grado/licenciatura/ingeniería
        /// </summary>
        Pregrado = 1,

        /// <summary>
        /// Trabajos de especialización y diplomados superiores
        /// </summary>
        Especializacion = 2,

        /// <summary>
        /// Tesis de maestría (mayor ponderación para promoción)
        /// </summary>
        Maestria = 3,

        /// <summary>
        /// Tesis doctoral (máxima ponderación para promoción)
        /// </summary>
        Doctorado = 4
    }

    public static class NivelAcademicoHelper
    {
        public static NivelAcademico ParseNivelAcademico(string nivelDto)
        {
            nivelDto = nivelDto?.Trim();
            switch (nivelDto)
            {
                case "Maestría":
                    nivelDto = "Maestria";
                    break;
                case "Especialización":
                    nivelDto = "Especializacion";
                    break;
                // Agrega más casos si hay otros valores con tildes o diferencias
            }

            Enum.TryParse<NivelAcademico>(nivelDto, true, out var nivelAcademico);
            return nivelAcademico;
        }
    }
}
