using MiApp.Application.DTOs;
using MiApp.Domain.Entities;
using MiApp.Domain.Interfaces;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MiApp.Application.Services
{
    /// <summary>
    /// Servicio de pagos - Maneja la lógica de negocio de pagos
    /// </summary>
    public class PaymentService
    {
        private readonly IPaymentRepository _paymentRepository;
        private readonly IOrderRepository _orderRepository;
        private readonly IMapper _mapper;

        public PaymentService(
            IPaymentRepository paymentRepository,
            IOrderRepository orderRepository,
            IMapper mapper)
        {
            _paymentRepository = paymentRepository;
            _orderRepository = orderRepository;
            _mapper = mapper;
        }

        /// <summary>
        /// Crear nuevo pago
        /// </summary>
        public async Task<PaymentDto> CreatePaymentAsync(CreatePaymentDto dto)
        {
            // Validar que la orden exista
            var order = await _orderRepository.GetByIdAsync(dto.OrderId);
            if (order == null)
                throw new InvalidOperationException($"La orden {dto.OrderId} no existe");

            // Crear entidad de pago
            var payment = new Payment
            {
                OrderId = dto.OrderId,
                Amount = dto.Amount,
                Method = dto.Method,
                Status = PaymentStatus.Pending,
                Currency = "ARS",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _paymentRepository.AddAsync(payment);
            await _paymentRepository.SaveChangesAsync();

            return _mapper.Map<PaymentDto>(payment);
        }

        /// <summary>
        /// Obtener pago por ID
        /// </summary>
        public async Task<PaymentDto> GetPaymentByIdAsync(int paymentId)
        {
            var payment = await _paymentRepository.GetByIdAsync(paymentId);
            if (payment == null)
                throw new InvalidOperationException($"El pago {paymentId} no existe");

            return _mapper.Map<PaymentDto>(payment);
        }

        /// <summary>
        /// Obtener pagos de una orden
        /// </summary>
        public async Task<List<PaymentDto>> GetPaymentsByOrderIdAsync(Guid orderId)
        {
            var payments = await _paymentRepository.GetByOrderIdAsync(orderId);
            return _mapper.Map<List<PaymentDto>>(payments);
        }

        /// <summary>
        /// Actualizar estado de pago
        /// </summary>
        public async Task<PaymentDto> UpdatePaymentStatusAsync(int paymentId, PaymentStatus status)
        {
            var payment = await _paymentRepository.UpdatePaymentStatusAsync(paymentId, status);
            if (payment == null)
                throw new InvalidOperationException($"El pago {paymentId} no existe");

            // Si el pago se completó, puedes agregar lógica adicional aquí
            // Como actualizar el estado de la orden, enviar notificaciones, etc.

            return _mapper.Map<PaymentDto>(payment);
        }

        /// <summary>
        /// Procesar pago con Mercado Pago
        /// </summary>
        public async Task<PaymentDto> CreateMercadoPagoPaymentAsync(Guid orderId, decimal amount)
        {
            var order = await _orderRepository.GetByIdAsync(orderId);
            if (order == null)
                throw new InvalidOperationException($"La orden {orderId} no existe");

            var payment = new Payment
            {
                OrderId = orderId,
                Amount = amount,
                Method = PaymentMethod.MercadoPago,
                Status = PaymentStatus.Processing,
                Currency = "ARS",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _paymentRepository.AddAsync(payment);
            await _paymentRepository.SaveChangesAsync();

            return _mapper.Map<PaymentDto>(payment);
        }

        /// <summary>
        /// Procesar pago con transferencia bancaria
        /// </summary>
        public async Task<PaymentDto> CreateBankTransferPaymentAsync(Guid orderId, decimal amount)
        {
            var order = await _orderRepository.GetByIdAsync(orderId);
            if (order == null)
                throw new InvalidOperationException($"La orden {orderId} no existe");

            var payment = new Payment
            {
                OrderId = orderId,
                Amount = amount,
                Method = PaymentMethod.BankTransfer,
                Status = PaymentStatus.Pending,
                Currency = "ARS",
                BankTransferId = Guid.NewGuid().ToString(),
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _paymentRepository.AddAsync(payment);
            await _paymentRepository.SaveChangesAsync();

            return _mapper.Map<PaymentDto>(payment);
        }

        /// <summary>
        /// Procesar pago con Uala
        /// </summary>
        public async Task<PaymentDto> CreateUalaPaymentAsync(Guid orderId, decimal amount)
        {
            var order = await _orderRepository.GetByIdAsync(orderId);
            if (order == null)
                throw new InvalidOperationException($"La orden {orderId} no existe");

            var payment = new Payment
            {
                OrderId = orderId,
                Amount = amount,
                Method = PaymentMethod.Uala,
                Status = PaymentStatus.Processing,
                Currency = "ARS",
                UalaTransactionId = Guid.NewGuid().ToString(),
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _paymentRepository.AddAsync(payment);
            await _paymentRepository.SaveChangesAsync();

            return _mapper.Map<PaymentDto>(payment);
        }
    }
}
