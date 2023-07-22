import { HmacManager } from "./hmac_manager/hmac-manager.js";
import { Request } from "node-fetch";
import { webcrypto } from "crypto";

global.crypto = webcrypto;

const clientId = "b9926638-6b5c-4a79-a6ca-014d8b848172";
const clientSecret = "11Nv/n22OqU59f9376E//I2rA2+Yg6yRaI0W6YRK/G0=";

const request = new Request("https://localhost:44363/api/people?id=1");
request.headers.append("X-AccountId", "myAccountId");
// request.headers.append("X-Email", "myemail@domain.com");

const hmacManager = new HmacManager(clientId, clientSecret, ["X-AccountId", "X-Email"]);
const hmacResult = await hmacManager.sign(request);

var debug = hmacResult;
console.dir(debug);