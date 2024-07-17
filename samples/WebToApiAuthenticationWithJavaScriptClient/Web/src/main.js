//import { HmacManager } from "../../../../scripts/hmac_manager/hmac-manager.js";
import { HmacManager } from "hmac-manager"
process.env.NODE_TLS_REJECT_UNAUTHORIZED = "0"

const publicKey = "b9926638-6b5c-4a79-a6ca-014d8b848172";
const privateKey = "11Nv/n22OqU59f9376E//I2rA2+Yg6yRaI0W6YRK/G0=";

const request = new Request("https://localhost:7216/api/weatherforecast");

// Add required headers for the scheme
request.headers.append("X-Account", "myAccountId");
request.headers.append("X-Email", "myemail@domain.com");

const hmacManager = new HmacManager(publicKey, privateKey, ["X-Account", "X-Email"]);
const hmacResult = await hmacManager.sign(request);

// Add required hmac headers
request.headers.append("X-Hmac-Policy", "MyPolicy");
request.headers.append("X-Hmac-Scheme", "RequireAccountAndEmail");
request.headers.append("X-Hmac-Date-Requested", `${hmacResult.hmac.dateRequested}`);
request.headers.append("X-Hmac-Nonce", `${hmacResult.hmac.nonce}`);

request.headers.append("Authorization", `Hmac ${hmacResult.hmac.signature}`);

console.dir(hmacResult);