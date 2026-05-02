<?php
// ===============================
// RUTA SOLO PARA ADMIN
// ===============================

// Incluir verificación JWT
require_once 'verificar_jwt.php';

// Validar token
$datosUsuario = verificarToken();

// ===============================
// VALIDAR ROL
// ===============================
if ($datosUsuario['rol'] !== 'admin') {
    http_response_code(403); // Prohibido
    echo json_encode([
        'error' => 'Acceso denegado. Solo administradores'
    ]);
    exit();
}

// ===============================
// RESPUESTA SI ES ADMIN
// ===============================
echo json_encode([
    'mensaje' => 'Bienvenido administrador',
    'usuario' => $datosUsuario['email'],
    'rol' => $datosUsuario['rol'],
    'datos_admin' => [
        'configuracion' => 'Panel de administración',
        'usuarios' => 'Lista de usuarios del sistema'
    ]
], JSON_PRETTY_PRINT);
?>