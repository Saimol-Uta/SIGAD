// Función para descargar archivos desde Blazor
window.downloadFile = (fileName, contentType, content) => {
    // Crear un blob con el contenido
    const blob = new Blob([content], { type: contentType });
    
    // Crear una URL temporal para el blob
    const url = window.URL.createObjectURL(blob);
    
    // Crear un elemento <a> temporal para la descarga
    const link = document.createElement('a');
    link.href = url;
    link.download = fileName;
    
    // Agregar el enlace al DOM, hacer clic y removerlo
    document.body.appendChild(link);
    link.click();
    document.body.removeChild(link);
    
    // Limpiar la URL temporal
    window.URL.revokeObjectURL(url);
};

// Función para mostrar notificaciones (opcional)
window.showNotification = (message, type = 'info') => {
    // Crear elemento de notificación
    const notification = document.createElement('div');
    notification.className = `notification notification-${type}`;
    notification.textContent = message;
    
    // Estilos básicos
    notification.style.cssText = `
        position: fixed;
        top: 20px;
        right: 20px;
        padding: 15px 20px;
        border-radius: 5px;
        color: white;
        font-weight: bold;
        z-index: 10000;
        max-width: 300px;
        word-wrap: break-word;
    `;
    
    // Colores según el tipo
    switch(type) {
        case 'success':
            notification.style.backgroundColor = '#28a745';
            break;
        case 'error':
            notification.style.backgroundColor = '#dc3545';
            break;
        case 'warning':
            notification.style.backgroundColor = '#ffc107';
            notification.style.color = '#212529';
            break;
        default:
            notification.style.backgroundColor = '#17a2b8';
    }
    
    // Agregar al DOM
    document.body.appendChild(notification);
    
    // Remover después de 5 segundos
    setTimeout(() => {
        if (document.body.contains(notification)) {
            document.body.removeChild(notification);
        }
    }, 5000);
}; 

// Función para hacer clic en un elemento por ID
window.clickElement = (elementId) => {
    const element = document.getElementById(elementId);
    if (element) {
        element.click();
    }
};