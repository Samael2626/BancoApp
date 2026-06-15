# Contribuir a BancoApp

Gracias por contribuir a BancoApp. Estas son las formas de colaborar:

## Cómo contribuir
1. Haz un fork del repositorio.
2. Crea una rama descriptiva:
   ```powershell
git checkout -b feature/nombre-descriptivo
```
3. Haz cambios pequeños y claros.
4. Añade pruebas o valida que la aplicación compile.
5. Envía un pull request hacia `main`.

## Estilo de commits
- Usa mensajes descriptivos.
- Prefiere el formato:
  - `feat:` para nuevas funcionalidades
  - `fix:` para correcciones
  - `docs:` para cambios en documentación
  - `chore:` para tareas menores

## Requisitos
- .NET SDK instalado
- Ejecuta `dotnet restore` antes de probar cambios

## Código limpio
- Mantén el código legible y estructurado.
- No incluyas datos sensibles ni claves en el repositorio.
