import { Hmac } from "./components/hmac.js"
import { HmacResultFactory } from "./components/hmac-result-factory.js"
import { HmacProvider } from "./components/hmac-provider.js"
import { HmacResult } from "./components/hmac-result.js";
import { HmacManagerOptions } from "./components/hmac-manager-options.js";

interface INonceCache {

}

export class HmacManager {
    private readonly options: HmacManagerOptions;
    private readonly provider: HmacProvider;
    private readonly signedHeaders: string[];
    private readonly cache: INonceCache;

    constructor(options: HmacManagerOptions, provider: HmacProvider) {
        this.options = options;
        this.provider = provider;
    }

    sign = async (request: Request): Promise<HmacResult> => {
        try {
            const hmac = new Hmac()
  
            hmac.signedHeaders  = this.signedHeaders
            hmac.signingContent = this.provider
                .computeSigningContent(request,
                    hmac.dateRequested,
                    hmac.nonce as string
                )

            hmac.signature = await this.provider
                .computeSignature(hmac.signingContent)

            return new HmacResultFactory(
                this.options.policy, 
                this.options.scheme.name
            ).create(hmac, hmac.signature !== null);
        } catch (error) {
            return new HmacResultFactory(
                this.options.policy, 
                this.options.scheme.name
            ).error(error?.message);
        }
    }
}