// loadtest/tasks_k6.js
import http from 'k6/http';
import { sleep, check } from 'k6';
import { Counter } from 'k6/metrics';

export const options = {
  stages: [
    { duration: '10s', target: 10 },
    { duration: '30s', target: 50 },
    { duration: '30s', target: 50 },
    { duration: '10s', target: 0 },
  ],
  thresholds: {
    http_req_duration: ['p(95)<300'],
    http_req_failed: ['rate<0.01'],
    'checks': ['rate>0.98'],
  },
};

const BASE = __ENV.BASE_URL || 'http://localhost:5135';
const httpErrors = new Counter('http_errors');

function record(res, name) {
  const ok = check(res, {
    [`${name} status is 2xx/3xx`]: (r) => r.status >= 200 && r.status < 400,
  });
  if (!ok) {
    httpErrors.add(1);
    console.error(`${name} failed: status=${res.status} body=${res.body?.substring(0, 200)}`);
  }
}

export default function () {
  const getRes = http.get(`${BASE}/api/tasks`);
  record(getRes, 'GET /api/tasks');

  const payload = JSON.stringify({ title: `k6-${__VU}-${Date.now()}`, description: 'perf' });
  const postRes = http.post(`${BASE}/api/tasks`, payload, { headers: { 'Content-Type': 'application/json' }});
  record(postRes, 'POST /api/tasks');

  sleep(1);
}