import { HmacResult } from "./components/hmac-result.js"
import { HmacProvider } from "./components/hmac-provider.js"

export class HmacManager {
    private readonly provider: HmacProvider
    private readonly signedHeaders: string[]

    constructor(
        publicKey: string,
        privateKey: string,
        signedHeaders: string[] = []
    ) {
        this.provider = new HmacProvider(
            publicKey, 
            privateKey, 
            signedHeaders
        )

        // These are required headers that are expected
        // to be passed into the sign method call. This can be null,
        // in which case no custom headers will be signed into the hmac.
        this.signedHeaders = signedHeaders;
    }

    sign = async (request: Request): Promise<HmacResult> => {
        try {
            const dateRequested = new Date();
            const nonce = crypto.randomUUID();
  
            const { signingContent, signature } = await
                this.provider.compute(request,
                    dateRequested,
                    nonce
                )

            return {
                hmac: {
                    dateRequested,
                    nonce,
                    signingContent,
                    signature,
                    signedHeaders: this.signedHeaders
                },
                isSuccess: true
            }
        } catch ({ message }) {
            return { isSuccess: false, message }
        }
    }
}