export default class DemoNotificationService {
    constructor(
        readonly apiUrl: string,
        readonly apiKey: string) {

    }

    async registerAsync(request: any): Promise<Response> {
        const registerApiUrl = `${this.apiUrl}/notifications/installations`;
        const result = await fetch(registerApiUrl, {
            method: 'PUT',
            headers: {
                Accept: 'application/json',
                'Content-Type': 'application/json',
                'apiKey': this.apiKey
            },
            body: JSON.stringify(request)
        });

        this.validateResponse(registerApiUrl, result);
        return result;
    }

    async deregisterAsync(deviceId: string): Promise<Response> {
        const deregisterApiUrl = `${this.apiUrl}/notifications/installations/${deviceId}`;
        const result = await fetch(deregisterApiUrl, {
            method: 'DELETE',
            headers: {
                Accept: 'application/json',
                'Content-Type': 'application/json',
                'apiKey': this.apiKey
            }
        });

        this.validateResponse(deregisterApiUrl, result);
        return result;
    }

    private validateResponse(requestUrl: string, response: Response) {
        console.log(`Request: ${requestUrl} => Response: ${response.status}`);
        if (!response || response.status != 200) {
            throw `HTTP error ${response.status}: ${response.statusText}`;
        }
    }
}