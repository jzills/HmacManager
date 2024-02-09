import { HmacManager } from "./hmac_manager/hmac-manager.js";
import { Request } from "node-fetch";
import { webcrypto } from "crypto";

global.crypto = webcrypto;

const publicKey = "b9926638-6b5c-4a79-a6ca-014d8b848172";
const privateKey = "11Nv/n22OqU59f9376E//I2rA2+Yg6yRaI0W6YRK/G0=";

const request = new Request("https://localhost:7197/home/protected");
request.headers.append("X-AccountId", "myAccountId");
request.headers.append("X-Email", "myemail@domain.com");

const hmacManager = new HmacManager(publicKey, privateKey);
const hmacResult = await hmacManager.sign(request);

console.dir(hmacResult);