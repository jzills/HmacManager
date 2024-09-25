import { SignatureBuilder } from "../builders/signature-builder.js"
import { SigningContentBuilder } from "../builders/signing-content-builder.js"
import { computeContentHash } from "../utilities/hmac-utilities.js";
import { HmacSignature } from "./hmac-signature.js";

export class HmacProvider {
    private readonly publicKey: string;
    private readonly privateKey: string;
    private readonly signedHeaders: string[] = [];
    private readonly contentHashAlgorithm: string = "sha-256";
    private readonly signatureHashAlgorithm: string = "sha-256";

    constructor(
        publicKey: string,
        privateKey: string,
        signedHeaders: string[] = [],
        contentHashAlgorithm: string = "sha-256",
        signatureHashAlgorithm: string = "sha-256"
    ) {
        this.publicKey = publicKey;
        this.privateKey = privateKey;
        this.signedHeaders = signedHeaders;
        this.contentHashAlgorithm = contentHashAlgorithm;
        this.signatureHashAlgorithm = signatureHashAlgorithm;
    }

    compute = async (request: Request, dateRequested: Date, nonce: string): Promise<HmacSignature> => {
        // Clone the request since we need to read the body if it exists
        const { method, body, headers, url } = request.clone();
        const { search, host, pathname } = new URL(url);

        const contentHash = await computeContentHash(body, this.contentHashAlgorithm);

        const signingContent = new SigningContentBuilder()
            .withMethod(method)
            .withPathAndQuery(`${pathname}${search}`)
            .withDateRequested(dateRequested)
            .withPublicKey(this.publicKey)
            .withContentHash(contentHash)
            .withSignedHeaders(this.signedHeaders, headers)
            .withNonce(nonce)
            .build();

        const signatureBuilder = new SignatureBuilder(
            this.privateKey,
            signingContent,
            this.signatureHashAlgorithm
        );

        return {
            signingContent,
            signature: await signatureBuilder.build()
        };
    }
}