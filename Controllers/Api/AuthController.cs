using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using StajPortal.Data;
using StajPortal.Models.DTOs;
using StajPortal.Models.Entities;
using StajPortal.Services;

namespace StajPortal.Controllers.Api
{

    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ApplicationDbContext _context;
        private readonly IJwtTokenService _jwtTokenService;

        public AuthController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            ApplicationDbContext context,
            IJwtTokenService jwtTokenService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
            _jwtTokenService = jwtTokenService;
        }

        [HttpPost("login")]
        [ProducesResponseType(typeof(AuthResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(AuthResponseDto), StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<AuthResponseDto>> Login([FromBody] LoginRequestDto request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new AuthResponseDto 
                { 
                    Success = false, 
                    Message = "Geçersiz giriş bilgileri" 
                });
            }

            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user == null || !user.IsActive)
            {
                return Unauthorized(new AuthResponseDto 
                { 
                    Success = false, 
                    Message = "Email veya şifre hatalı" 
                });
            }

            var result = await _signInManager.CheckPasswordSignInAsync(user, request.Password, lockoutOnFailure: true);

            if (!result.Succeeded)
            {
                return Unauthorized(new AuthResponseDto 
                { 
                    Success = false, 
                    Message = result.IsLockedOut ? "Hesabınız kilitlendi" : "Email veya şifre hatalı" 
                });
            }

            var token = _jwtTokenService.GenerateToken(user);
            var expiresAt = DateTime.UtcNow.AddHours(24);

            return Ok(new AuthResponseDto
            {
                Success = true,
                Message = "Giriş başarılı",
                Token = token,
                ExpiresAt = expiresAt,
                User = new UserDto
                {
                    Id = user.Id,
                    Email = user.Email!,
                    FullName = user.FullName ?? "",
                    Role = user.Role
                }
            });
        }
        [HttpPost("register")]
        [ProducesResponseType(typeof(AuthResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(AuthResponseDto), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<AuthResponseDto>> Register([FromBody] RegisterRequestDto request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new AuthResponseDto 
                { 
                    Success = false, 
                    Message = "Geçersiz kayıt bilgileri" 
                });
            }

            var existingUser = await _userManager.FindByEmailAsync(request.Email);
            if (existingUser != null)
            {
                return BadRequest(new AuthResponseDto 
                { 
                    Success = false, 
                    Message = "Bu email adresi zaten kullanılıyor" 
                });
            }

            var user = new ApplicationUser
            {
                UserName = request.Email,
                Email = request.Email,
                FullName = request.FullName,
                Role = request.Role,
                CreatedAt = DateTime.UtcNow,
                IsActive = true,
                EmailConfirmed = true
            };

            var result = await _userManager.CreateAsync(user, request.Password);

            if (!result.Succeeded)
            {
                return BadRequest(new AuthResponseDto 
                { 
                    Success = false, 
                    Message = string.Join(", ", result.Errors.Select(e => e.Description)) 
                });
            }

            // Profil oluştur
            if (request.Role == "Student")
            {
                var studentProfile = new StudentProfile
                {
                    UserId = user.Id,
                    FullName = request.FullName,
                    UpdatedAt = DateTime.UtcNow
                };
                _context.StudentProfiles.Add(studentProfile);
            }
            else if (request.Role == "Company")
            {
                var companyProfile = new CompanyProfile
                {
                    UserId = user.Id,
                    CompanyName = request.FullName,
                    UpdatedAt = DateTime.UtcNow
                };
                _context.CompanyProfiles.Add(companyProfile);
            }

            await _context.SaveChangesAsync();

            // Role Claim ekle
            await _userManager.AddClaimAsync(user, new System.Security.Claims.Claim("Role", request.Role));

            var token = _jwtTokenService.GenerateToken(user);
            var expiresAt = DateTime.UtcNow.AddHours(24);

            return Ok(new AuthResponseDto
            {
                Success = true,
                Message = "Kayıt başarılı",
                Token = token,
                ExpiresAt = expiresAt,
                User = new UserDto
                {
                    Id = user.Id,
                    Email = user.Email,
                    FullName = user.FullName ?? "",
                    Role = user.Role
                }
            });
        }
    }
}

