import { Hmac } from "./hmac.js"

export type HmacResult = {
    policy: string,
    scheme: string | null,
    hmac: Hmac | null,
    isSuccess: boolean,
    dateGenerated: Date
}