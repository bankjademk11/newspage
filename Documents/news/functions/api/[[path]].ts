
// functions/api/[[path]].ts

export const onRequest: PagesFunction<{ VITE_NEWS_API_KEY: string }> = async ({ request, env, params }) => {
  const url = new URL(request.url);
  const path = (params.path as string[]).join('/');
  const targetUrl = new URL(`https://newsapi.org/v2/${path}${url.search}`);

  // Append the API key from environment variables
  targetUrl.searchParams.set('apiKey', env.VITE_NEWS_API_KEY);

  // Create a new request to the NewsAPI endpoint
  const newRequest = new Request(targetUrl.toString(), {
    method: request.method,
    headers: {
      ...request.headers,
      'Origin': 'https://newsapi.org',
      'Referer': 'https://newsapi.org',
      'User-Agent': 'Cloudflare-Pages-Proxy/1.0'
    },
    body: request.body,
    redirect: 'follow'
  });

  try {
    const response = await fetch(newRequest);
    
    // Create a new response, modifying headers to allow CORS
    const newResponse = new Response(response.body, response);
    newResponse.headers.set('Access-Control-Allow-Origin', '*');
    newResponse.headers.set('Access-Control-Allow-Methods', 'GET, POST, PUT, DELETE, OPTIONS');
    newResponse.headers.set('Access-Control-Allow-Headers', 'Content-Type, Authorization');

    return newResponse;

  } catch (error) {
    return new Response(JSON.stringify({ error: 'Proxy failed', details: error.message }), {
      status: 500,
      headers: { 'Content-Type': 'application/json' },
    });
  }
};
