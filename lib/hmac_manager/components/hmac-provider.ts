import { SignatureBuilder } from "../builders/signature-builder.js"
import { SigningContentBuilder } from "../builders/signing-content-builder.js"

export type HmacSignature = { signingContent: string, signature: string };

export class HmacProvider {
    private readonly publicKey: string
    private readonly privateKey: string
    private readonly signedHeaders: string[] = []
    
    constructor(
        publicKey: string,
        privateKey: string,
        signedHeaders: string[] = []
    ) {
        this.publicKey = publicKey
        this.privateKey = privateKey
        this.signedHeaders = signedHeaders
    }

    compute = async (request: Request, dateRequested: Date, nonce: string): Promise<HmacSignature> => {
        const { headers, method, body, url } = request 
        const { pathname, search, host } = new URL(url)
        const signingContent = new SigningContentBuilder()
            .withMethod(method)
            .withPathAndQuery(`${pathname}${search}`)
            .withAuthority(host)
            .withRequested(dateRequested)
            .withPublicKey(this.publicKey)
            .withBody(body)
            .withNonce(nonce)
            .withSignedHeaders(this.signedHeaders, headers)
            .build()

        const signatureBuilder = new SignatureBuilder(this.privateKey, signingContent);
        
        return {
            signingContent,
            signature: await signatureBuilder.build()
        }
    }
}