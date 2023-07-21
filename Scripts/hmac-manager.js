class HmacManager {
    constructor(appId, appSecret) {
        this.appId = appId;
        this.appSecret = appSecret;
    }

    Sign = async ({ method, body }) => {
        const nonce = crypto.randomUUID();
        const requestedOn = new Date();
        var clientSecretBytes = Uint8Array.from(clientSecret, c => c.charCodeAt(0));
        var keyBytes = await crypto.subtle.importKey("raw", clientSecretBytes, { name: "HMAC", hash: "SHA-256" }, false, ["sign"]);
        var signingContent = "GET:/endpoint?id=1:www.myapi.com:7/20/2023 9:16:44 PM +00:00:b9926638-6b5c-4a79-a6ca-014d8b848172:65ee5f71-efdd-4370-80d7-ec733d572bcf";
        var signingContentBytes = Uint8Array.from(signingContent, c => c.charCodeAt(0));
        var signatureBytes = await crypto.subtle.sign("HMAC", keyBytes, signingContentBytes);
        var bytes = new Uint8Array(signatureBytes);
        btoa(bytes.toString().split(",").map(x => String.fromCharCode(x)).join(""));
    }

    ComputeContentHash = content => {

    }
}