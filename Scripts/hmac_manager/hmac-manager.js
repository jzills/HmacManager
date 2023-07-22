import { Hmac } from "./components/hmac.js"
import { HmacResult } from "./components/hmac-result.js"
import { HmacProvider } from "./components/hmac-provider.js"

export class HmacManager {
    #provider = null

    constructor(clientId, clientSecret, signedHeaders) {
        this.#provider = new HmacProvider(
            clientId, 
            clientSecret, 
            signedHeaders
        )

        // These are required headers that are expected
        // to be passed into the sign method call. This can be null,
        // in which case no custom headers will be signed into the hmac.
        this.signedHeaders = signedHeaders;
    }

    sign = async request => {
        try {
            const hmac = new Hmac()
        
            hmac.signedHeaders  = this.signedHeaders
            hmac.signingContent = this.#provider
                .computeSigningContent(request,
                    hmac.requestedOn,
                    hmac.nonce
                )

            hmac.signature = await this.#provider
                .computeSignature(hmac.signingContent)

            return new HmacResult(hmac, true)
        } catch (error) {
            return new HmacResult(null, false, error.message)
        }
    }
}