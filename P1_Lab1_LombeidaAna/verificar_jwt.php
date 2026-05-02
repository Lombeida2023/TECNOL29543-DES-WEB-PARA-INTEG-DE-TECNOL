<?php
// ===============================
// ARCHIVO PARA VALIDAR EL JWT
// ===============================

// Cargar librerías instaladas con Composer
require_once 'vendor/autoload.php';

// Cargar configuración (clave secreta, etc.)
require_once 'config.php';

// Importar clases necesarias de la librería JWT
use Firebase\JWT\JWT;
use Firebase\JWT\Key;

/**
 * FUNCIÓN: verificarToken()
 * ----------------------------------
 * Verifica si el token enviado por el cliente es válido.
 *
 * Si es válido → retorna los datos del usuario
 * Si no → devuelve error y detiene ejecución
 */
function verificarToken() {

    // ===============================
    //  1. INTENTAR OBTENER TOKEN POR URL (GET)
    // ===============================
    // Esto se usa como alternativa cuando los headers fallan en XAMPP
    if (isset($_GET['token'])) {
        $token = $_GET['token'];
    } else {
        $token = null;
    }

    // ===============================
    //  2. INTENTAR OBTENER TOKEN DESDE HEADERS
    // ===============================
    if (!$token) {

        // Obtener headers si la función existe
        $headers = function_exists('getallheaders') ? getallheaders() : [];

        // Buscar Authorization (mayúscula)
        if (isset($headers['Authorization'])) {
            $token = $headers['Authorization'];
        } 
        // Buscar authorization (minúscula)
        elseif (isset($headers['authorization'])) {
            $token = $headers['authorization'];
        }
    }

    // ===============================
    // 3. INTENTAR DESDE $_SERVER (XAMPP FIX)
    // ===============================
    if (!$token && isset($_SERVER['HTTP_AUTHORIZATION'])) {
        $token = $_SERVER['HTTP_AUTHORIZATION'];
    }

    if (!$token && isset($_SERVER['REDIRECT_HTTP_AUTHORIZATION'])) {
        $token = $_SERVER['REDIRECT_HTTP_AUTHORIZATION'];
    }

    // ===============================
    // VALIDAR SI EXISTE TOKEN
    // ===============================
    if (!$token) {
        http_response_code(401);
        echo json_encode(['error' => 'Token no proporcionado']);
        exit();
    }

    // ===============================
    // 🔑 LIMPIAR TOKEN (QUITAR "Bearer ")
    // ===============================
    $token = str_replace('Bearer ', '', $token);

    // ===============================
    // 🔐 VALIDAR TOKEN
    // ===============================
    try {

        // Decodificar token usando la clave secreta
        $decoded = JWT::decode(
            $token,
            new Key(JWT_SECRET, JWT_ALGORITHM)
        );

        // Retornar datos del usuario como array
        return (array) $decoded;

    } catch (\Firebase\JWT\ExpiredException $e) {

        http_response_code(401);
        echo json_encode(['error' => 'Token expirado. Inicia sesión nuevamente']);
        exit();

    } catch (\Firebase\JWT\SignatureInvalidException $e) {

        http_response_code(401);
        echo json_encode(['error' => 'Token inválido o alterado']);
        exit();

    } catch (Exception $e) {

        http_response_code(401);
        echo json_encode(['error' => 'Error: ' . $e->getMessage()]);
        exit();
    }
}
?>