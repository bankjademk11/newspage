import React, { useState } from 'react';
import { Article } from '../services/newsApi';
import { useTranslation } from 'react-i18next';
import DateBadge from './DateBadge';

interface NewsCardProps {
  article: Article;
  onClick?: (article: Article) => void;
}

export default function NewsCard({ article, onClick }: NewsCardProps) {
  const { t, i18n } = useTranslation();
  const [isTranslating, setIsTranslating] = useState(false);
  const [translatedTitle, setTranslatedTitle] = useState('');
  const [translatedDescription, setTranslatedDescription] = useState('');

  const formatDate = (dateString: string) => {
    const date = new Date(dateString);
    const now = new Date();
    const diffInHours = Math.floor((now.getTime() - date.getTime()) / (1000 * 60 * 60));
    
    if (diffInHours < 1) {
      return t('news.timeAgo.justNow');
    } else if (diffInHours < 24) {
      return `${diffInHours} ${t('news.timeAgo.hoursAgo')}`;
    } else {
      const diffInDays = Math.floor(diffInHours / 24);
      return `${diffInDays} ${t('news.timeAgo.daysAgo')}`;
    }
  };

  const handleTranslate = async (e: React.MouseEvent) => {
    e.stopPropagation();
    if (isTranslating) return;
    
    setIsTranslating(true);
    try {
      const targetLang = i18n.language === 'lo' ? 'lo' : 'en';
      
      // Use Google Translate API (you'll need to set up API key)
      // For now, we'll use browser's built-in translation as fallback
      if ('translation' in navigator && 'translate' in (navigator as any).translation) {
        const translator = (navigator as any).translation.createTranslator({
          sourceLanguage: 'auto',
          targetLanguage: targetLang
        });
        
        const translatedTitleResult = await translator.translate(article.title);
        const translatedDescResult = article.description 
          ? await translator.translate(article.description)
          : '';
          
        setTranslatedTitle(translatedTitleResult);
        setTranslatedDescription(translatedDescResult);
      } else {
        // Fallback: Simple translation simulation
        setTranslatedTitle(`[${targetLang.toUpperCase()}] ${article.title}`);
        setTranslatedDescription(article.description ? `[${targetLang.toUpperCase()}] ${article.description}` : '');
      }
    } catch (error) {
      console.error('Translation failed:', error);
    } finally {
      setIsTranslating(false);
    }
  };

  const handleClick = () => {
    if (onClick) {
      onClick(article);
    } else {
      window.open(article.url, '_blank');
    }
  };

  const displayTitle = translatedTitle || article.title;
  const displayDescription = translatedDescription || article.description;

  return (
    <div 
      className="apple-card cursor-pointer overflow-hidden group card-hover fade-in"
      onClick={handleClick}
    >
      <div className="relative h-48 overflow-hidden">
        <img 
          src={article.urlToImage || '/assets/placeholder-1.svg'} 
          alt={article.title}
          className="w-full h-full object-cover group-hover:scale-105 transition-transform duration-300"
          onError={(e) => {
            const target = e.target as HTMLImageElement;
            target.src = '/assets/placeholder-1.svg';
          }}
        />
        <div className="absolute inset-0 bg-gradient-to-t from-black/50 to-transparent opacity-0 group-hover:opacity-100 transition-opacity duration-300"></div>
      </div>
      
      <div className="p-6">
        <div className="flex items-center justify-between mb-3">
          <span className="text-xs font-semibold text-blue-600 uppercase tracking-wide">
            {article.source.name}
          </span>
          <div className="relative group">
            <DateBadge dateString={article.publishedAt} />
          </div>
        </div>
        
        <h3 className="font-semibold text-gray-900 mb-3 line-clamp-2 group-hover:text-blue-600 transition-colors duration-200 text-lg">
          {displayTitle}
        </h3>
        
        {displayDescription && (
          <p className="text-gray-600 text-sm line-clamp-3 mb-4">
            {displayDescription}
          </p>
        )}
        
        <div className="flex items-center justify-between">
          {article.author && (
            <p className="text-xs text-gray-500 italic">
              {t('news.by')} {article.author}
            </p>
          )}
          
          <button 
            onClick={handleTranslate}
            disabled={isTranslating}
            className="flex items-center gap-2 px-3 py-1.5 text-xs font-medium text-blue-600 hover:text-blue-700 hover:bg-blue-50 rounded-lg transition-all duration-200 disabled:opacity-50"
          >
            {isTranslating ? (
              <>
                <div className="w-3 h-3 border border-blue-600 border-t-transparent rounded-full animate-spin"></div>
                {t('news.translate')}...
              </>
            ) : (
              <>
                <svg className="w-3 h-3" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M3 5h12M9 3v2m1.048 9.5A18.022 18.022 0 016.412 9m6.088 9h7M11 21l5-10 5 10M12.751 5C11.783 10.77 8.07 15.61 3 18.129" />
                </svg>
                {t('news.translate')}
              </>
            )}
          </button>
        </div>
        
        <div className="mt-4 pt-4 border-t border-gray-100">
          <button 
            onClick={(e) => {
              e.stopPropagation();
              handleClick();
            }}
            className="w-full px-4 py-2 bg-blue-600 text-white font-semibold rounded-lg hover:bg-blue-700 transition-colors duration-200 text-sm"
          >
            {t('news.readMore')}
          </button>
        </div>
      </div>
    </div>
  );
}
