<?php
// ===============================
// LOGIN - GENERA TOKEN JWT
// ===============================

// Cargar librerías y configuración
require_once 'vendor/autoload.php';
require_once 'config.php';

// Importar clase JWT
use Firebase\JWT\JWT;

// Indicar que la respuesta será en formato JSON
header('Content-Type: application/json');

// ===============================
// VALIDAR MÉTODO HTTP
// ===============================

// Solo se permite método POST
if ($_SERVER['REQUEST_METHOD'] !== 'POST') {
    http_response_code(405);
    echo json_encode(['error' => 'Usa método POST']);
    exit();
}

// ===============================
// LEER DATOS DEL CLIENTE
// ===============================

// Leer el body en formato JSON
$data = json_decode(file_get_contents('php://input'), true);

// Validar que existan email y password
if (!isset($data['email']) || !isset($data['password'])) {
    http_response_code(400);
    echo json_encode(['error' => 'Faltan email o password']);
    exit();
}

// ===============================
// BASE DE DATOS SIMULADA
// ===============================

// Usuarios de prueba (simulan una BD)
$usuarios = [
    [
        'id' => 1,
        'email' => 'admin@taller.com',
        'password' => '1234',
        'rol' => 'admin'
    ],
    [
        'id' => 2,
        'email' => 'usuario@taller.com',
        'password' => 'abcd',
        'rol' => 'user'
    ],
    [
        'id' => 3,
        'email' => 'editor@taller.com',
        'password' => '1234',
        'rol' => 'editor'
    ]
];

// ===============================
// BUSCAR USUARIO
// ===============================

$usuarioEncontrado = null;

foreach ($usuarios as $user) {
    // Comparar email y password
    if (
        $user['email'] === $data['email'] &&
        $user['password'] === $data['password']
    ) {
        $usuarioEncontrado = $user;
        break;
    }
}

// Si no existe usuario válido
if (!$usuarioEncontrado) {
    http_response_code(401);
    echo json_encode(['error' => 'Credenciales incorrectas']);
    exit();
}

// ===============================
// CREAR TOKEN JWT
// ===============================

// Payload = datos que irán dentro del token
$payload = [
    'iss' => 'P1_Lab1_LombeidaAna', // Emisor
    'iat' => time(),               // Fecha de creación
    'exp' => time() + JWT_EXPIRATION, // Expiración
    'user_id' => $usuarioEncontrado['id'],
    'email' => $usuarioEncontrado['email'],
    'rol' => $usuarioEncontrado['rol'],
];

// Generar el token firmado
$token = JWT::encode($payload, JWT_SECRET, JWT_ALGORITHM);

// ===============================
// RESPUESTA FINAL
// ===============================

echo json_encode([
    'mensaje' => 'Login exitoso',
    'token' => $token,
    'expira' => date('Y-m-d H:i:s', $payload['exp'])
]);
?>