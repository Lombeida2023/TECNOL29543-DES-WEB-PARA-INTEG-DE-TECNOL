<?php
// ===============================
// RUTA PROTEGIDA CON JWT
// ===============================

// Incluir función que valida el token
require_once 'verificar_jwt.php';

// Indicar respuesta JSON
header('Content-Type: application/json');

// ===============================
// VALIDAR TOKEN
// ===============================

// Si el token es inválido, la función termina el script automáticamente
$datosUsuario = verificarToken();

// ===============================
// RESPUESTA SI TODO ESTÁ BIEN
// ===============================

// Si llegó aquí → usuario autenticado
echo json_encode([
    'mensaje' => 'Acceso permitido',
    'usuario' => $datosUsuario['email'],
    'rol' => $datosUsuario['rol'],
    'fecha_acceso' => date('Y-m-d H:i:s'),

    // Datos simulados protegidos
    'datos_secretos' => [
        'info1' => 'Solo usuarios autenticados pueden ver esto',
        'info2' => 'JWT funcionando correctamente'
    ]
]);
?>