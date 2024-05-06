export class HmacResult {
    hmac;
    isSuccess;
    error;
    constructor(hmac, isSuccess, error) {
        this.hmac = hmac;
        this.isSuccess = isSuccess;
        this.error = error;
    }
}
