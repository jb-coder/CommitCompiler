
namespace CommitCompilerShared.Models
{
    public class BuildConfiguration
    {
        public int Id { get; set; }  // Clave primaria

        // Campos requeridos
        public string Organization { get; set; }

        public string Project { get; set; }           // Nombre del proyecto
        public string Repository { get; set; }        // URL o path del repositorio
        public string Branch { get; set; }               // Rama de origen
        public DateTime DateStartProcess { get; set; }  // Fecha inicio del proceso
        public DateTime DateEndProcess { get; set; }     // Fecha fin del proceso
        public double ProcessTime { get; set; }         // Tiempo total en minutos

        // Configuraciones adicionales
        public bool AutoMerge { get; set; }            // True si el autoMerge está habilitado
        public string? DestinationBranch { get; set; }        // Rama destino para el merge

        // Notificaciones
        public bool Notification { get; set; }         // True si las notificaciones están habilitadas
        public string EmailOriginSender { get; set; }        // Correo electrónico del origen (remitente)
        public string EmailOriginPass { get; set; }  // Contraseña del email origen
        public string EmailDestination { get; set; }       // Correo electrónico del destino (destinatario)
        public string EmailDestinationSubject { get; set; }        // Asunto del correo electrónico
        public string NotificationSubjectCommit { get; set; }  // Asunto de notificación al subir un commit

        // Path de destino para guardar la compilación
        public string PathDestination { get; set; }        // Ruta donde se guardará la compilación
        public string Token { get; set; }
    }

}
