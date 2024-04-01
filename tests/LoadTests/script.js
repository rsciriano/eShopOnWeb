// https://k6.io/docs/using-k6/test-lifecycle/
// 1. init code
import { uuidv4 } from 'https://jslib.k6.io/k6-utils/1.4.0/index.js';
import generateJwt from "./helpers/jwt-generator.js";
import { Home } from "./resourceObjectModel/web/home.js";
import { Basket } from "./resourceObjectModel/web/basket.js";

import { htmlReport } from "https://raw.githubusercontent.com/benc-uk/k6-reporter/main/dist/bundle.js";

export const options = {
    tags : {
      testid : `eshop-${new Date().toJSON()}`
    },
    scenarios: {
        ramping: {
          executor: 'ramping-vus',
          startVUs: 1,
          stages: [
            { duration: '1m', target: 100 },
            { duration: '10m', target: 100 },
          ],
          gracefulRampDown: '0s',
        },
    }
 };

export function setup() {
    // 2. setup code
}
  
export default function (data) {
// 3. VU code
const baseUrl = 'http://localhost:5106'
const userId = uuidv4();
//const auth = generateJwt(userId);
const auth = `LoadTests ${userId}`;
console.debug('Start iteration with UserId: ', userId)

const home = new Home(baseUrl, auth);
const basket = new Basket(baseUrl, auth);

home.load();
home.addToBasket();
home.addToBasket();

basket.load();
basket.checkout();
}

export function teardown(data) {
// 4. teardown code
}

export function handleSummary(data) {
  return {
    "summary.html": htmlReport(data),
  };
}