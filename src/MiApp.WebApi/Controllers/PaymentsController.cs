using MiApp.Application.DTOs;
using MiApp.Application.Services;
using MiApp.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MiApp.WebApi.Controllers
{
    /// <summary>
    /// Controlador de Pagos
    /// Maneja endpoints relacionados con procesamiento de pagos
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class PaymentsController : ControllerBase
    {
        private readonly PaymentService _paymentService;
        private readonly ILogger<PaymentsController> _logger;

        public PaymentsController(
            PaymentService paymentService,
            ILogger<PaymentsController> logger)
        {
            _paymentService = paymentService;
            _logger = logger;
        }

        /// <summary>
        /// Obtener información de pago por ID
        /// </summary>
        /// <param name="id">ID del pago</param>
        [HttpGet("{id}")]
        public async Task<ActionResult<PaymentDto>> GetPaymentById(int id)
        {
            try
            {
                var payment = await _paymentService.GetPaymentByIdAsync(id);
                return Ok(payment);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex.Message);
                return NotFound(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Obtener todos los pagos de una orden
        /// </summary>
        /// <param name="orderId">ID de la orden</param>
        [HttpGet("order/{orderId}")]
        public async Task<ActionResult<List<PaymentDto>>> GetPaymentsByOrderId(Guid orderId)
        {
            try
            {
                var payments = await _paymentService.GetPaymentsByOrderIdAsync(orderId);
                return Ok(payments);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error obteniendo pagos de orden");
                return BadRequest(new { message = "Error obteniendo pagos" });
            }
        }

        /// <summary>
        /// Crear preferencia de pago en Mercado Pago
        /// </summary>
        /// <param name="request">Datos de la orden y monto</param>
        [HttpPost("mercado-pago/preference")]
        public async Task<ActionResult<MercadoPagoPreferenceDto>> CreateMercadoPagoPreference(
            [FromBody] CreatePaymentDto request)
        {
            try
            {
                // Validar datos
                if (request.Amount <= 0)
                    return BadRequest(new { message = "Datos inválidos" });

                // Crear pago
                var payment = await _paymentService.CreateMercadoPagoPaymentAsync(
                    request.OrderId,
                    request.Amount);

                // En un entorno real, aquí integrarías con la API de Mercado Pago
                // Para este ejemplo, devolvemos URLs de demostración

                var preference = new MercadoPagoPreferenceDto
                {
                    Id = payment.Id.ToString(),
                    PreferenceId = "dummy_preference_" + Guid.NewGuid(),
                    Amount = payment.Amount,
                    OrderId = payment.OrderId,
                    // URLs de demostración
                    InitPoint = "https://checkout.mercadopago.com/checkout/v1/redirect?preference-id=dummy",
                    SandboxInitPoint = "https://sandbox.mercadopago.com/checkout/v1/redirect?preference-id=dummy"
                };

                return Ok(preference);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creando preferencia de Mercado Pago");
                return BadRequest(new { message = "Error procesando pago" });
            }
        }

        /// <summary>
        /// Webhook para notificaciones de Mercado Pago
        /// </summary>
        [HttpPost("mercado-pago/webhook")]
        [AllowAnonymous]
        public async Task<ActionResult> MercadoPagoWebhook([FromBody] MercadoPagoWebhookDto notification)
        {
            try
            {
                if (notification?.Type != "payment")
                    return Ok();

                if (notification?.Data?.Id == null)
                {
                    _logger.LogWarning("Webhook recibido sin ID de pago");
                    return BadRequest(new { message = "ID de pago requerido" });
                }

                _logger.LogInformation($"Webhook recibido para pago: {notification.Data.Id}");

                // Aquí irían las validaciones con Mercado Pago
                // y la actualización del estado del pago

                return Ok(new { success = true });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error procesando webhook de Mercado Pago");
                return Ok(); // Devolver 200 de todas formas para no reintentar
            }
        }

        /// <summary>
        /// Crear pago con transferencia bancaria
        /// </summary>
        /// <param name="request">Datos de transferencia bancaria</param>
        [HttpPost("bank-transfer")]
        public async Task<ActionResult<PaymentDto>> CreateBankTransfer(
            [FromBody] CreatePaymentDto request)
        {
            try
            {
                var payment = await _paymentService.CreateBankTransferPaymentAsync(
                    request.OrderId,
                    request.Amount);

                return Ok(payment);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex.Message);
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creando transferencia bancaria");
                return BadRequest(new { message = "Error procesando pago" });
            }
        }

        /// <summary>
        /// Crear pago con Uala
        /// </summary>
        /// <param name="request">Datos de pago Uala</param>
        [HttpPost("uala")]
        public async Task<ActionResult<PaymentDto>> CreateUalaPayment(
            [FromBody] CreatePaymentDto request)
        {
            try
            {
                var payment = await _paymentService.CreateUalaPaymentAsync(
                    request.OrderId,
                    request.Amount);

                return Ok(payment);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex.Message);
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creando pago Uala");
                return BadRequest(new { message = "Error procesando pago" });
            }
        }
    }
}
