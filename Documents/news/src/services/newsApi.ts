import axios from 'axios';

// NewsAPI configuration
const NEWS_API_KEY = import.meta.env.VITE_NEWS_API_KEY || '58ab77b0d7d94b45ac0270d527c44ff2'; // API key จาก https://newsapi.org/
const NEWS_API_BASE_URL = import.meta.env.VITE_API_BASE_URL || 'https://newsapi.org/v2/';

// สร้าง instance สำหรับเรียก API
const newsApi = axios.create({
  baseURL: NEWS_API_BASE_URL,
  headers: {
    'Accept': 'application/json',
    'Content-Type': 'application/json',
  },
  timeout: 10000,
});

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
        apiKey: NEWS_API_KEY,
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
        apiKey: NEWS_API_KEY,
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
      params: { category, country: 'us', apiKey: NEWS_API_KEY },
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
