import React, { useState, useEffect } from 'react';
import { getTopHeadlines, Article } from '../services/newsApi';
import { useTranslation } from 'react-i18next';

export default function BreakingNewsTicker() {
  const { t } = useTranslation();
  const [articles, setArticles] = useState<Article[]>([]);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    const fetchBreakingNews = async () => {
      try {
        const response = await getTopHeadlines();
        // Take first 8 articles for ticker
        setArticles(response.articles.slice(0, 8));
      } catch (error) {
        console.error('Error fetching breaking news:', error);
      } finally {
        setLoading(false);
      }
    };

    fetchBreakingNews();
  }, []);

  if (loading || articles.length === 0) {
    return (
      <div className="bg-red-600 text-white py-2 overflow-hidden">
        <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
          <div className="flex items-center gap-4">
            <span className="bg-white text-red-600 px-3 py-1 rounded text-xs font-bold uppercase whitespace-nowrap">
              {t('news.breaking')}
            </span>
            <div className="animate-pulse">
              <div className="h-4 bg-red-500 rounded w-48"></div>
            </div>
          </div>
        </div>
      </div>
    );
  }

  return (
    <div className="bg-red-600 text-white py-2 overflow-hidden relative">
      <div className="absolute left-0 top-0 bottom-0 bg-red-700 z-10 flex items-center px-4">
        <span className="bg-white text-red-600 px-3 py-1 rounded text-xs font-bold uppercase whitespace-nowrap">
          {t('news.breaking')}
        </span>
      </div>
      
      <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
        <div className="flex items-center">
          <div className="w-24"></div> {/* Spacer for the breaking label */}
          <div className="overflow-hidden flex-1">
            <div className="animate-marquee whitespace-nowrap">
              {articles.map((article, index) => (
                <span key={index} className="inline-block mx-8">
                  <span className="font-medium">{article.title}</span>
                  <span className="mx-2 text-red-200">•</span>
                  <span className="text-red-200 text-sm">
                    {article.source.name}
                  </span>
                </span>
              ))}
            </div>
          </div>
        </div>
      </div>

      <style>{`
        @keyframes marquee {
          0% {
            transform: translateX(0%);
          }
          100% {
            transform: translateX(-50%);
          }
        }
        
        .animate-marquee {
          display: inline-block;
          animation: marquee 30s linear infinite;
        }
        
        .animate-marquee:hover {
          animation-play-state: paused;
        }
      `}</style>
    </div>
  );
}
