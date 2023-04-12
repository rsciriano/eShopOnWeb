// https://k6.io/docs/using-k6/test-lifecycle/
// 1. init code
import { uuidv4 } from 'https://jslib.k6.io/k6-utils/1.4.0/index.js';
import { Home } from "./resourceObjectModel/web/home.js";
import { Basket } from "./resourceObjectModel/web/basket.js";

export const options = {
  tags : {
    testid : `eshop-${new Date().toJSON()}`
  },
  scenarios: {
      ramping: {
        executor: 'ramping-vus',
        startVUs: 1,
        stages: [
          { duration: '1m', target: 25 },
          { duration: '10m', target: 25 },
        ],
        gracefulRampDown: '0s',
      },
  }
};

// 2. setup code
export function setup() {
}
  
// 3. VU code
export default function (data) {
  const baseUrl = 'https://eshop-web.yellowstone-459f3d9b.westeurope.azurecontainerapps.io'
  const userId = uuidv4();
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

// 4. teardown code
export function teardown(data) {

}
