import { SignatureBuilder } from "../builders/signature-builder.js"
import { SigningContentBuilder } from "../builders/signing-content-builder.js"
import { computeContentHash } from "../utilities/hmac-utilities.js";

export type HmacSignature = { signingContent: string, signature: string };

export class HmacProvider {
    private readonly publicKey: string;
    private readonly privateKey: string;
    private readonly signedHeaders: string[] = [];

    constructor(
        publicKey: string,
        privateKey: string,
        signedHeaders: string[] = []
    ) {
        this.publicKey = publicKey;
        this.privateKey = privateKey;
        this.signedHeaders = signedHeaders;
    }

    compute = async (request: Request, dateRequested: Date, nonce: string): Promise<HmacSignature> => {
        const { method, body, headers, url } = request;
        const { search, host, pathname } = new URL(url);

        const contentHash = await computeContentHash(body, "sha-256");

        const signingContent = new SigningContentBuilder()
            .withMethod(method)
            .withPathAndQuery(`${pathname}${search}`)
            .withAuthority(host)
            .withDateRequested(dateRequested)
            .withPublicKey(this.publicKey)
            .withContentHash(contentHash)
            .withSignedHeaders(this.signedHeaders, headers)
            .withNonce(nonce)
            .build();

        const signatureBuilder = new SignatureBuilder(this.privateKey, signingContent);

        return {
            signingContent,
            signature: await signatureBuilder.build()
        };
    }
}