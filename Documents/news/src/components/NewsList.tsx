import React, { useState, useEffect } from 'react';
import { getTopHeadlines, getNewsByCategory, Article, NEWS_CATEGORIES } from '../services/newsApi';
import NewsCard from './NewsCard';

interface NewsListProps {
  category?: string;
  searchQuery?: string;
}

export default function NewsList({ category, searchQuery }: NewsListProps) {
  const [articles, setArticles] = useState<Article[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  const fetchNews = async () => {
    setLoading(true);
    setError(null);
    
    try {
      let response;
      if (searchQuery) {
        // ถ้ามีการค้นหา จะใช้ฟังก์ชันค้นหา (ต้อง import searchNews)
        const { searchNews } = await import('../services/newsApi');
        response = await searchNews(searchQuery);
      } else if (category) {
        response = await getNewsByCategory(category);
      } else {
        response = await getTopHeadlines();
      }
      
      setArticles(response.articles);
    } catch (err) {
      setError('ไม่สามารถดึงข้อมูลข่าวได้ กรุณาลองใหม่อีกครั้ง');
      console.error('Error fetching news:', err);
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    fetchNews();
  }, [category, searchQuery]);

  if (loading) {
    return (
      <div className="flex justify-center items-center py-12">
        <div className="animate-spin rounded-full h-12 w-12 border-b-2 border-blue-600"></div>
      </div>
    );
  }

  if (error) {
    return (
      <div className="text-center py-12">
        <div className="text-red-600 mb-4">
          <svg className="w-16 h-16 mx-auto mb-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M12 8v4m0 4h.01M21 12a9 9 0 11-18 0 9 9 0 0118 0z" />
          </svg>
        </div>
        <p className="text-gray-600 mb-4">{error}</p>
        <button 
          onClick={fetchNews}
          className="px-6 py-2 bg-blue-600 text-white rounded-lg hover:bg-blue-700 transition-colors duration-200"
        >
          ลองใหม่
        </button>
      </div>
    );
  }

  if (articles.length === 0) {
    return (
      <div className="text-center py-12">
        <div className="text-gray-400 mb-4">
          <svg className="w-16 h-16 mx-auto mb-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M19 20H5a2 2 0 01-2-2V6a2 2 0 012-2h10a2 2 0 012 2v1m2 13a2 2 0 01-2-2V7m2 13a2 2 0 002-2V9a2 2 0 00-2-2h-2m-4-3H9M7 16h6M7 8h6v4H7V8z" />
          </svg>
        </div>
        <p className="text-gray-600">ไม่พบข่าวที่คุณกำลังหา</p>
      </div>
    );
  }

  return (
    <div className="space-y-6">
      <div className="flex items-center justify-between">
        <h2 className="text-2xl font-bold text-gray-900">
          {searchQuery ? `Search Results: "${searchQuery}"` : 
           category ? NEWS_CATEGORIES.find(c => c.value === category)?.label || 'News' : 
           'Latest News'}
        </h2>
        <span className="text-sm text-gray-500">
          Found {articles.length} articles
        </span>
      </div>
      
      <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
        {articles.map((article, index) => (
          <NewsCard 
            key={`${article.url}-${index}`} 
            article={article}
          />
        ))}
      </div>
    </div>
  );
}
