<?php
// ===============================
// CONFIGURACIÓN SEGURA JWT
// ===============================

// Clave secreta 
define('JWT_SECRET', 'Ana_Lombeida_ClaveSuperSegura_2026_$#@!_JWT_LAB');

// Tiempo de vida del token (1 hora)
define('JWT_EXPIRATION', 3600);

// Algoritmo de firma
define('JWT_ALGORITHM', 'HS256');
?>