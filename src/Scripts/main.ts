import { HmacManager } from "./HmacManager/hmac-manager.js";
import { HmacResult } from "./HmacManager/components/hmac-result.js";

try {
    const pubKey = "a18f5729-32ce-43a4-ac4d-af0a699539ae";
    const priKey = "xCy0Ucg3YEKlmiK23Zph+g==";

    const hmacManager = new HmacManager(pubKey, priKey, []);

    const request = new Request("https://localhost:7197/home/protected");
    request.headers.append("X-AccountId", "myAccountId");
    request.headers.append("X-Email", "myemail@domain.com");
    const siginingResult = await hmacManager.sign(request);
    console.dir(siginingResult);
} catch (error) {
    console.log("Error");
    console.log(error);
}