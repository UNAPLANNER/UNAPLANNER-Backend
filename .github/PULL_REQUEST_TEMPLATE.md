# 📥 Pull Request Template

## 📌 Descripción
*Ej: Implementación de login con JWT + pantalla en Compose.*

## ✅ Cambios principales
- **[Backend .NET]:** Controladores, servicios, validación de credenciales y generación de JWT.
- **[Frontend Android]:** Retrofit, sesión segura, pantalla en Compose y navegación.
- **[Limpieza]:** `.gitignore` actualizado.

## 📋 Reglas de negocio
- [ ] Usuario existe en BD.
- [ ] Contraseña validada con hash seguro.
- [ ] JWT incluye `userId` y `role`.

## 🧪 Pruebas
- [ ] Backend funcionando (Postman/Swagger).
- [ ] Flujo completo desde Android.
- [ ] Manejo de errores (credenciales inválidas, red, etc.).

## 📱 Endpoints
| Método | Endpoint | Uso |
|--------|----------|-----|
| POST | `/api/auth/login` | Autenticar usuario |

## 📝 Notas (opcional)
*Algo importante que el revisor deba saber.*