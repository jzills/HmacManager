type SigningContentContext = {
    request: Request | null; 
    publicKey: string; 
    dateRequested: Date; 
    nonce: string;
    signedHeaders: string[];
    contentHash: string | null; 
}


export default SigningContentContext;