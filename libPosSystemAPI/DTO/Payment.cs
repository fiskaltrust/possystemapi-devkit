using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace fiskaltrust.Payment.DTO
{
    /// <summary>
    /// The currently know protocols we have payment providers for.
    /// NOTE: none, use_auto, use_first are not real protocols but rather special cases to handle the payment provider selection automatically.
    /// IMPORTANT: The test protocol is used for testing purposes only (e.g. Unit tests) and should not be used in production environments.
    /// </summary>
    public enum PaymentProtocol
    {
        none,
        use_auto,
        use_first,
        gp_softpos_gptom,       // using GP top SoftPOS app (note: When using InStore App DEBUG build the "GP tom Dev" / sandbox app use targeted; if a production build is used the "GP tom" production app is targeted)
        bluecode,               // currently DISABLED - see #170
        gp_mpas,                // GlobalPayments via hardware terminal (used PAX A920Pro during development)
        payone_softpos_wpi,     // Worldline Tap on Mobile
        viva_eft_pos_instore,   // viva.com Terminal / Viva Wallet
        hobex_softpos_posit,    // not supported anymore as the old solution was cancelled by Hobex and the new one (based on softpay.io) is not yet implemented by us
        hobexecr,               // HobexECR (hardware terminal; tested on Sunmi V2 Pro)
        softpayio,              // softpay.io SoftPOS app
        shift4,                 // Shift4 hardware terminal (PAX A800)

#if DEBUG
        /// <summary>
        /// Do not use this protocol in production environments! It is only used for testing purposes.
        /// </summary>
        test,
        /// <summary>
        /// Do not use this protocol in production environments! It is only used for testing purposes.
        /// Used to test the error handling in the payment state machine.
        /// </summary>
        test_result_handler_throws_exception,
#endif
    }

    public enum PayAction
    {
        payment,
        // not supported by all payment providers
        cancel,
        refund,
        /* not yet supported:
        pre_authorization,
        other
        */
    }
}
