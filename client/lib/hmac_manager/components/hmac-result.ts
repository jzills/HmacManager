import { Hmac } from "./hmac.js"

export type HmacResult = {
    isSuccess: boolean,
    message?: string
    hmac?: Hmac | null,
}