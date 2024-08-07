export type Hmac = {
    dateRequested: Date;
    nonce: string;
    signingContent: string;
    signedHeaders: string[] | null;
    signature: string;
}