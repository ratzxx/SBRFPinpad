using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace SBRFPinpad
{
    public enum Function
    {
        IntCardPurchase = 4000, // Покупка
        IntCardRefund = 4002, // Возврат покупки
        IntCardTotals = 6000, // Итоги дня
        IntCardXReport = 6002, // Х-отчет по картам с магнитной полосой
        IntCancelTransaction = 6004, // отмена операции
        IntPinpadReady = 13,
        IntAddInfo = 7005 // Установить дополнительные реквизиты платежа (например, номера авиабилетов)
    }

    public class SBRFPinpad : IDisposable
    {
        private dynamic _pinpad;

        public SBRFPinpad()
        {
            Init();
        }

        private void Init()
        {
            // Run in separate thread to prevent System.ArithmeticException. SBRF changes FPU CW register
            var t1 = Task.Factory.StartNew(() =>
            {
                try
                {
                    var type = Type.GetTypeFromProgID("SBRFSRV.Server");
                    _pinpad = Activator.CreateInstance(type);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
            });
            t1.Wait();
        }

        public virtual bool PinpadReady()
        {
            return _pinpad != null && _pinpad.NFun(Function.IntPinpadReady) == 0;
        }

        /// <summary>
        /// Возвращает номер терминала
        /// </summary>
        public virtual string GetTermNum()
        {
            return _pinpad.GParamString("TermNum");
        }

        /// <summary>
        /// Возвращает номер карты
        /// </summary>
        public virtual string GetClientCard()
        {
            return _pinpad.GParamString("ClientCard");
        }

        public virtual string GetClientExpiryDate()
        {
            return _pinpad.GParamString("ClientExpiryDate");
        }

        public virtual string GetAuthCode()
        {
            return _pinpad.GParamString("AuthCode");
        }

        /// <summary>
        /// Возвращает имя карты, например Mastercard
        /// </summary>
        public virtual string GetCardName()
        {
            return _pinpad.GParamString("CardName");
        }

        public virtual string GetCheque()
        {
            return _pinpad.GParamString("Cheque");
        }

        public void Clear()
        {
            _pinpad.Clear();
        }

        public virtual void SetAmount(int amount)
        {
            _pinpad.SParam("Amount", amount);
        }

        /// <summary>
        /// Внесение в чек дополнительных реквизитов платежа
        /// </summary>
        public virtual void SetPayInfo(string info)
        {
            _pinpad.SParam("PayInfo", info);
            _pinpad.NFun(Function.IntAddInfo);
        }

        /// <summary>
        /// Precision = 2
        /// </summary>
        public virtual void SetAmount(decimal amount)
        {
            var rounded = Math.Round(amount, 2);
            var cents = rounded * 100;
            var result = Convert.ToInt32(cents);
            SetAmount(result);
        }

        /// <summary>
        /// NFun(4000)
        /// </summary>
        public virtual int Purchase()
        {
            return _pinpad.NFun(Function.IntCardPurchase);
        }

        /// <summary>
        /// NFun(4002)
        /// </summary>
        public virtual int Refund()
        {
            return _pinpad.NFun(Function.IntCardRefund);
        }

        /// <summary>
        /// Если операция была успешно выполнена, но печать чека выполнить не удалось, или если на чеке имеется графа «Подпись клиента», а клиент отказался подписывать чек, то операцию следует отменить 
        /// </summary>
        public virtual int CancelTransaction()
        {
            return _pinpad.NFun(Function.IntCancelTransaction);
        }

        /// <summary>
        /// Итоги дня по картам с магнитной полосой 
        /// </summary>
        public virtual int ZReport()
        {
            return _pinpad.NFun(Function.IntCardTotals);
        }

        /// <summary>
        /// Х-отчет по картам с магнитной полосой
        /// </summary>
        public virtual int XReport()
        {
            return _pinpad.NFun(Function.IntCardXReport);
        }

        public virtual void Dispose()
        {
            if (_pinpad != null)
            {
                Marshal.ReleaseComObject(_pinpad);
            }
        }
    }
}
