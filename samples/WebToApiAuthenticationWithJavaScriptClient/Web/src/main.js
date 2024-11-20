import { HmacManagerFactory } from "hmac-manager"

// For demo purposes...
process.env.NODE_TLS_REJECT_UNAUTHORIZED = "0";

const URL = "https://localhost:7216/api/weatherforecast";

// Initialize HmacManagerFactory with full set
// of policies and schemes.
const hmacManagerFactory = new HmacManagerFactory([{
    name: "MyPolicy",
    publicKey: "b9926638-6b5c-4a79-a6ca-014d8b848172",
    privateKey: "11Nv/n22OqU59f9376E//I2rA2+Yg6yRaI0W6YRK/G0=",
    contentHashAlgorithm: "sha-256",
    signatureHashAlgorithm: "sha-256",
    schemes: [{
        name: "RequireAccountAndEmail",
        headers: ["X-Account", "X-Email"]
    }]
}]);

// Create an instance of HmacManager.
const hmacManager = hmacManagerFactory.create("MyPolicy", "RequireAccountAndEmail");

const request = samplePostRequest();
//const request = sampleGetRequest();

// Sign the request, this will automatically add the authorization
// header including any other additional headers used by authentication.
const hmacResult = await hmacManager.sign(request);
const response = await fetch(request);

console.assert(hmacResult.isSuccess);
console.assert(response.ok);

console.dir(hmacResult);
console.dir(await response.json());

function sampleGetRequest() {
    return addRequiredHeaders(new Request(URL));
}

function samplePostRequest() {
    return addRequiredHeaders(new Request(URL, {
        method: "post",
        headers: { "content-type": "application/json" },
        body: JSON.stringify({ summary: "This is a test." })
    }));
}

function addRequiredHeaders(request) {
    request.headers.append("X-Account", "myAccountId");
    request.headers.append("X-Email", "myemail@domain.com");
    return request;
}