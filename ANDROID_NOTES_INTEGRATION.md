# GET /api/student/{id}/notes - Documentación para Android

## Descripción
Obtiene todas las notas de un estudiante específico. Opcionalmente puedes filtrar por curso.

## Endpoint
```
GET /api/student/{id}/notes?courseId={courseId}
```

## Parámetros
- `id` (path): ID del estudiante (requerido)
- `courseId` (query): ID del curso para filtrar notas (opcional)

## Respuestas

### 200 OK - Éxito
```json
{
  "data": [
    {
      "id": 1,
      "title": "Título de la nota",
      "content": "Contenido de la nota",
      "courseId": 5,
      "createdAt": "2026-05-06T10:30:00",
      "updatedAt": "2026-05-06T10:30:00"
    },
    {
      "id": 2,
      "title": "Otra nota",
      "content": "Más contenido",
      "courseId": null,
      "createdAt": "2026-05-05T14:20:00",
      "updatedAt": "2026-05-05T14:20:00"
    }
  ],
  "message": "Notas obtenidas exitosamente"
}
```

### 400 Bad Request - Error
```json
{
  "message": "Estudiante con ID 999 no encontrado."
}
```

## Ejemplos de uso

### Desde Android (Retrofit)
```kotlin
// Interface de Retrofit
@GET("/api/student/{id}/notes")
suspend fun getStudentNotes(
    @Path("id") studentId: Int,
    @Query("courseId") courseId: Int? = null
): Response<NotesResponse>

// En el ViewModel/Repository
try {
    val response = apiService.getStudentNotes(studentId, courseId)
    if (response.isSuccessful) {
        val notes = response.body()?.data
        // Procesar notas
    } else {
        // Manejar error
    }
} catch (e: Exception) {
    // Manejar excepción
}
```

### Ejemplos de URLs
- Obtener todas las notas: `GET /api/student/1/notes`
- Filtrar por curso: `GET /api/student/1/notes?courseId=5`

## Modelos de datos

### NoteResponse
```json
{
  "id": 1,
  "title": "string",
  "content": "string",
  "courseId": "int (nullable)",
  "createdAt": "datetime",
  "updatedAt": "datetime"
}
```

## Validaciones
- El estudiante con el ID proporcionado debe existir (caso contrario devuelve error 400)
- Si se proporciona courseId, se filtran las notas por ese curso
- Las notas se devuelven ordenadas por fecha de creación (más recientes primero)

## Instalación en Android Studio

### 1. Definir Retrofit Interface
```kotlin
interface ApiService {
    @GET("/api/student/{id}/notes")
    suspend fun getStudentNotes(
        @Path("id") studentId: Int,
        @Query("courseId") courseId: Int? = null
    ): Response<StudentNotesResponse>
}
```

### 2. Crear Data Classes
```kotlin
data class StudentNotesResponse(
    val data: List<NoteResponse>,
    val message: String
)

data class NoteResponse(
    val id: Int,
    val title: String,
    val content: String?,
    val courseId: Int?,
    val createdAt: String,
    val updatedAt: String
)
```

### 3. Llamar desde ViewModel
```kotlin
viewModelScope.launch {
    try {
        val response = apiService.getStudentNotes(studentId)
        if (response.isSuccessful) {
            _notes.value = response.body()?.data ?: emptyList()
        }
    } catch (e: Exception) {
        _error.value = e.message
    }
}
```
