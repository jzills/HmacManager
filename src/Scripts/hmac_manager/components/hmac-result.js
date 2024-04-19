export class HmacResult {
    constructor(hmac, isSuccess, error) {
        this.hmac = hmac
        this.isSuccess = isSuccess
        this.error = error
    }
}