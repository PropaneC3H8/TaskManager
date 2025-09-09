import http from 'k6/http';
import { sleep, check } from 'k6';

export const options = {
  stages: [
    { duration: '10s', target: 10 },   // ramp to 10 VUs
    { duration: '30s', target: 50 },   // ramp to 50 VUs
    { duration: '30s', target: 50 },   // hold
    { duration: '10s', target: 0 },    // ramp down
  ],
  thresholds: {
    http_req_duration: ['p(95)<300'],  // 95% under 300ms
    http_req_failed: ['rate<0.01'],    // <1% errors
  },
};

const BASE = __ENV.BASE_URL || 'http://localhost:5135';

export default function () {
  // GET all
  const getRes = http.get(`${BASE}/api/tasks`);
  check(getRes, { 'GET /api/tasks is 200': (r) => r.status === 200 });

  // POST one (lightweight payload)
  const payload = JSON.stringify({ title: `k6-${__VU}-${Date.now()}`, description: 'perf' });
  const postRes = http.post(`${BASE}/api/tasks`, payload, { headers: { 'Content-Type': 'application/json' }});
  check(postRes, { 'POST /api/tasks created': (r) => r.status === 201 });

  sleep(1);
}
