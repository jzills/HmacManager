export type Algorithm = { name: string, hash: string };

export class SignatureBuilder {
    private readonly privateKey: string
    private readonly signingContent: string
    private readonly algorithm: Algorithm

    constructor(
        privateKey: string,
        signingContent: string,
        algorithm: Algorithm = { name: "HMAC", hash: "SHA-256" }
    ) {
        this.privateKey = privateKey
        this.signingContent = signingContent
        this.algorithm = algorithm
    }

    build = async () => {

        const assertValidBuild = () => {
            const isMissingRequiredValues = 
                this.privateKey     === null || 
                this.signingContent === null
            
            if (isMissingRequiredValues) {
                throw new Error("Required values are missing.")
            }
        }

        assertValidBuild();

        const getByteArray = (content: string) => 
            Uint8Array.from(content, 
                element => element.charCodeAt(0))

        const getUnicodeForm = (signatureBytes: ArrayBuffer) => {
            const bytes = new Uint8Array(signatureBytes)
            const bytesSplit = bytes.toString().split(",")
            const unicodeForm = bytesSplit.map(element => String.fromCharCode(parseInt(element))).join("")
            return unicodeForm
        }

        const getKeyBytes = async () => crypto.subtle.importKey("raw", 
            getByteArray(atob(this.privateKey)), 
            this.algorithm, 
            false, 
            ["sign"]
        )

        const getSignature = async (keyBytes: CryptoKey, signingContentBytes: BufferSource) => 
            crypto.subtle.sign("HMAC", keyBytes, signingContentBytes)

        const signatureBytes = await getSignature(await 
            getKeyBytes(), 
            getByteArray(this.signingContent)
        )

        const unicodeForm = getUnicodeForm(signatureBytes)
        return btoa(unicodeForm)
    }
}