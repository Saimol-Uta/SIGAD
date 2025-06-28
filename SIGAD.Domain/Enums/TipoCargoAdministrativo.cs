namespace SIGAD.Domain.Enums
{
    /// <summary>
    /// Tipos de cargos administrativos según Art. 7 del reglamento
    /// Los cargos de autoridad aplican excepcionalidades completas
    /// </summary>
    public enum TipoCargoAdministrativo
    {
        // 🔴 Cargos de autoridad (excepcionalidades completas Art. 7)

        /// <summary>
        /// Rector de universidad o escuela politécnica (excepcionalidades completas)
        /// </summary>
        Rector = 1,

        /// <summary>
        /// Vicerrector académico o administrativo (excepcionalidades completas)
        /// </summary>
        Vicerrector = 2,

        /// <summary>
        /// Autoridad de organismo público del SNES (excepcionalidades completas)
        /// </summary>
        AutoridadSNES = 3,

        // 🟡 Cargos de gestión educativa (Art. 3c - beneficios parciales)

        /// <summary>
        /// Director de carrera o programa académico
        /// </summary>
        DirectorCarrera = 4,

        /// <summary>
        /// Coordinador de programa de posgrado
        /// </summary>
        CoordinadorPrograma = 5,

        /// <summary>
        /// Jefe de departamento académico
        /// </summary>
        JefeDepartamento = 6,

        /// <summary>
        /// Director de centro de investigación
        /// </summary>
        DirectorCentro = 7,

        /// <summary>
        /// Secretario académico institucional
        /// </summary>
        SecretarioAcademico = 8,

        /// <summary>
        /// Coordinador de investigación institucional
        /// </summary>
        CoordinadorInvestigacion = 9,

        /// <summary>
        /// Decano de facultad
        /// </summary>
        Decano = 10,

        // 🟢 Participaciones externas (equivalencias Art. 3f,g,h)

        /// <summary>
        /// Miembro externo de comisión de evaluación (16 horas equivalencia)
        /// </summary>
        MiembroComisionExterno = 11,

        /// <summary>
        /// Facilitador del CES (24-32 horas equivalencia)
        /// </summary>
        FacilitadorCES = 12,

        /// <summary>
        /// Evaluador del CACES (32 horas equivalencia)
        /// </summary>
        EvaluadorCACES = 13,

        /// <summary>
        /// Par evaluador externo (16 horas equivalencia)
        /// </summary>
        ParEvaluadorExterno = 14,

        /// <summary>
        /// Consultor especializado para organismos públicos
        /// </summary>
        ConsultorEspecializado = 15
    }
}
