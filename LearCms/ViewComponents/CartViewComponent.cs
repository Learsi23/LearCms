using LearCms.Contexts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http; // Asegúrate de que esto esté ahí

public class CartViewComponent : ViewComponent
{
    private readonly ApplicationDbContext _context;
    // 🚨 Mejor práctica: Usar const o readonly static para constantes, como lo haces en el Controller.
    private const string CartSessionId = "CartSessionId";

    public CartViewComponent(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IViewComponentResult> InvokeAsync()
    {
        var sessionId = GetSessionId();

        var cartItemCount = await _context.CartItems
            .Where(c => c.SessionId == sessionId)
            .SumAsync(c => c.Quantity);

        return View("Default", cartItemCount);
    }

    private string GetSessionId()
    {
        // ✅ CORRECCIÓN CLAVE: Acceder a la sesión a través de Request.HttpContext.Session
        var session = HttpContext.Request.HttpContext.Session;

        if (string.IsNullOrEmpty(session.GetString(CartSessionId)))
        {
            session.SetString(CartSessionId, Guid.NewGuid().ToString());
        }
        // Retorna el valor (usando el operador null-forgiving (!) si estás en C# 8+)
        return session.GetString(CartSessionId)!;
    }
}