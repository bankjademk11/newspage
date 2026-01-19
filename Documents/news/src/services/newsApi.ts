import axios from 'axios';

// ใช้ proxy สำหรับ development และ production
const NEWS_API_BASE_URL = '/api';

// สร้าง instance สำหรับเรียก API
const newsApi = axios.create({
  baseURL: NEWS_API_BASE_URL,
  headers: {
    'Accept': 'application/json',
    'Content-Type': 'application/json',
    'Access-Control-Allow-Origin': '*',
  },
  withCredentials: false,
  timeout: 15000,
});

// แก้ไข preflight request issue
newsApi.interceptors.request.use((config) => {
  // ถ้าเป็น development ให้เพิ่ม headers พิเศษ
  if (import.meta.env.DEV) {
    config.headers['X-Requested-With'] = 'XMLHttpRequest';
  }
  return config;
});

// แก้ไข response error handling
newsApi.interceptors.response.use(
  (response) => response,
  async (error) => {
    // ถ้าเป็น CORS error ให้ลองใช้ CORS proxy
    if (error.code === 'ERR_NETWORK' || error.message?.includes('CORS')) {
      console.warn('CORS error detected, trying fallback...');
      // สามารถเพิ่ม fallback logic ที่นี่
    }
    return Promise.reject(error);
  }
);

// Interface สำหรับข่าว
export interface Article {
  source: {
    id: string | null;
    name: string;
  };
  author: string | null;
  title: string;
  description: string | null;
  url: string;
  urlToImage: string | null;
  publishedAt: string;
  content: string | null;
}

export interface NewsResponse {
  status: string;
  totalResults: number;
  articles: Article[];
}

// ฟังก์ชันดึงข่าวล่าสุด
export const getTopHeadlines = async (category?: string): Promise<NewsResponse> => {
  try {
    const params = category ? { category } : {};
    const response = await newsApi.get('/top-headlines', { 
      params: {
        ...params,
        country: 'us', // เปลี่ยนเป็น 'us' เพราะข่าวไทยมีน้อย
        pageSize: 20,
      }
    });
    return response.data;
  } catch (error) {
    console.error('Error fetching top headlines:', error);
    throw error;
  }
};

// ฟังก์ชันค้นหาข่าว
export const searchNews = async (query: string): Promise<NewsResponse> => {
  try {
    const response = await newsApi.get('/everything', {
      params: {
        q: query,
        sortBy: 'publishedAt',
        language: 'th',
        domains: 'bangkokpost.com,thaipbsworld.com,nationthailand.com,khaosod.co.th,matichon.co.th,thairath.co.th,manager.co.th',
      },
    });
    return response.data;
  } catch (error) {
    console.error('Error searching news:', error);
    throw error;
  }
};

// ฟังก์ชันดึงข่าวตามหมวดหมู่
export const getNewsByCategory = async (category: string): Promise<NewsResponse> => {
  try {
    const response = await newsApi.get('/top-headlines', {
      params: { category, country: 'us' },
    });
    return response.data;
  } catch (error) {
    console.error('Error fetching news by category:', error);
    throw error;
  }
};

// หมวดหมู่ข่าวที่รองรับ
export const NEWS_CATEGORIES = [
  { value: 'business', label: 'ธุรกิจ' },
  { value: 'entertainment', label: 'บันเทิง' },
  { value: 'general', label: 'ทั่วไป' },
  { value: 'health', label: 'สุขภาพ' },
  { value: 'science', label: 'วิทยาศาสตร์' },
  { value: 'sports', label: 'กีฬา' },
  { value: 'technology', label: 'เทคโนโลยี' },
];
